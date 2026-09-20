import { test, expect } from '@playwright/test';

test('Doctor login → patients → view/edit → delete attempt is forbidden', async ({ page, request }) => {
  await page.goto('/login');

  await page.getByTestId('login-username').fill('doctor');
  await page.getByTestId('login-password').fill('Doctor123!');
  await page.getByTestId('login-submit').click();

  await expect(page).toHaveURL(/\/patients$/);
  await expect(page.getByTestId('current-user')).toContainText('doctor (Doctor)');

  // Doctor can create
  await page.getByTestId('patient-create').click();
  const uniqueLastName = `E2E-Doctor-${Date.now()}`;
  await page.getByTestId('patient-firstName').fill('DoctorFlow');
  await page.getByTestId('patient-lastName').fill(uniqueLastName);
  await page.getByTestId('patient-dateOfBirth').fill('1985-02-20');
  await page.getByTestId('patient-gender').click();
  await page.getByRole('option', { name: 'Female' }).click();
  await page.getByTestId('patient-form-submit').click();

  await expect(page).toHaveURL(/\/patients\/(\d+)$/);
  const url = page.url();
  const patientId = url.match(/\/patients\/(\d+)$/)?.[1];

  // Doctor can edit
  await page.getByTestId('patient-detail-edit').click();
  await page.getByTestId('patient-firstName').fill('DoctorFlowUpdated');
  await page.getByTestId('patient-form-submit').click();
  await expect(page.getByText(`DoctorFlowUpdated ${uniqueLastName}`)).toBeVisible();

  // UI must not expose a delete action for Doctor role
  await expect(page.getByTestId('patient-detail-delete')).toHaveCount(0);

  // Server-side authorization must independently reject a direct DELETE call
  const token = await page.evaluate(() => localStorage.getItem('pm_access_token'));
  const deleteResponse = await request.delete(`http://localhost:5080/api/patients/${patientId}`, {
    headers: { Authorization: `Bearer ${token}` }
  });
  expect(deleteResponse.status()).toBe(403);
});
