import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import NotFound from '../page/NotFound/NotFound';

describe('NotFound Component (404 Page)', () => {
  it('renders the 404 error code and header correctly', () => {
    render(
      <BrowserRouter>
        <NotFound />
      </BrowserRouter>
    );

    expect(screen.getByText('Error 404')).toBeInTheDocument();
    expect(screen.getByText('404')).toBeInTheDocument();
    expect(screen.getByText(/Lost in Space\? Page Not Found/i)).toBeInTheDocument();
  });

  it('renders navigation action buttons back to home and catalog', () => {
    render(
      <BrowserRouter>
        <NotFound />
      </BrowserRouter>
    );

    const homeLink = screen.getByRole('link', { name: /Back to Home/i });
    expect(homeLink).toBeInTheDocument();
    expect(homeLink).toHaveAttribute('href', '/');

    const catalogLink = screen.getByRole('link', { name: /Browse Products/i });
    expect(catalogLink).toBeInTheDocument();
    expect(catalogLink).toHaveAttribute('href', '/category');
  });

  it('updates document.title on mount', () => {
    render(
      <BrowserRouter>
        <NotFound />
      </BrowserRouter>
    );

    expect(document.title).toBe('LuxeStore | 404 - Page Not Found');
  });
});
