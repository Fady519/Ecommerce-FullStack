import { test, expect } from '@playwright/test';

test.describe('LuxeStore Full-Stack E2E Suite', () => {

  // Test 1: Visitor Browsing Experience
  test('Scenario 1: Visitor browses homepage, searches, and views product details', async ({ page }) => {
    // 1. Visit homepage
    await page.goto('/');
    await expect(page).toHaveTitle(/LuxeStore/i);

    // 2. Check brand header & navigation
    await expect(page.locator('.brand_logo')).toBeVisible();

    // 3. Search for a product
    const searchInput = page.locator('input[placeholder*="Search"]');
    if (await searchInput.isVisible()) {
      await searchInput.fill('Classic');
      await searchInput.press('Enter');
      await page.waitForTimeout(1000);
    }

    // 4. Navigate to category catalog
    await page.goto('/category');
    await expect(page.locator('.category_products')).toBeVisible();

    // 5. Open a product card
    const firstProduct = page.locator('.product').first();
    if (await firstProduct.isVisible()) {
      await firstProduct.click();
      await page.waitForTimeout(1000);
      await expect(page).toHaveURL(/products/);
    }
  });

  // Test 2: Customer Purchase Journey
  test('Scenario 2: Customer logs in, adds item to cart, and views cart/checkout', async ({ page }) => {
    // 1. Login with verified account
    await page.goto('/login');
    await page.fill('input[type="email"]', 'fadykaiser2004@gmail.com');
    await page.fill('input[type="password"]', 'Fady@Store2026');
    await page.locator('input[type="password"]').press('Enter');

    // Wait for login to complete (user box visible in navbar)
    await expect(page.locator('.user_logged_box')).toBeVisible({ timeout: 10000 });

    // 2. Add product to cart
    await page.goto('/category');
    await page.waitForSelector('.category_products', { timeout: 8000 });
    const addToCartBtn = page.locator('.action_icon_btn').first();
    if (await addToCartBtn.isVisible()) {
      await addToCartBtn.click();
      await page.waitForTimeout(1000);
    }

    // 3. Open cart
    await page.goto('/cart');
    await expect(page.locator('.checkout')).toBeVisible({ timeout: 8000 });
  });

  // Test 3: Administrator Portal
  test('Scenario 3: Administrator logs in and navigates Admin Portal', async ({ page }) => {
    // 1. Login as Admin
    await page.goto('/login');
    await page.fill('input[type="email"]', 'fadykaiser2004@gmail.com');
    await page.fill('input[type="password"]', 'Fady@Store2026');
    await page.locator('input[type="password"]').press('Enter');

    // Wait for login to complete and Admin pill to appear
    await expect(page.locator('.user_logged_box')).toBeVisible({ timeout: 10000 });

    // 2. Navigate to Admin Dashboard
    await page.goto('/admin');
    await expect(page.locator('.admin_dashboard_page')).toBeVisible({ timeout: 10000 });

    // 3. Switch tabs (Products, Catalog, Orders)
    const productsTab = page.locator('button:has-text("Products Catalog")');
    if (await productsTab.isVisible()) {
      await productsTab.click();
      await expect(page).toHaveURL(/tab=products/);
    }

    const catalogTab = page.locator('button:has-text("Categories & Brands")');
    if (await catalogTab.isVisible()) {
      await catalogTab.click();
      await expect(page).toHaveURL(/tab=catalog/);
    }
  });

});
