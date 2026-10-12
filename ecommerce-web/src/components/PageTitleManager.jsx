import { useEffect } from 'react';
import { useLocation } from 'react-router-dom';

const routeTitles = {
  '/': 'LuxeStore | Premium E-Commerce Store',
  '/login': 'LuxeStore | Sign In',
  '/register': 'LuxeStore | Create Account',
  '/search': 'LuxeStore | Search Results',
  '/category': 'LuxeStore | Products Catalog',
  '/about': 'LuxeStore | About Us & Architecture',
  '/contact': 'LuxeStore | Contact & Customer Support',
  '/blog': 'LuxeStore | Journal & Editorial',
  '/accessories': 'LuxeStore | Accessories Collection',
  '/cart': 'LuxeStore | Shopping Cart',
  '/checkout': 'LuxeStore | Secure Checkout',
  '/orders': 'LuxeStore | My Orders History',
  '/favorites': 'LuxeStore | My Wishlist',
  '/admin': 'LuxeStore | Admin Operations Dashboard',
};

export default function PageTitleManager() {
  const location = useLocation();

  useEffect(() => {
    const path = location.pathname;
    
    if (routeTitles[path]) {
      document.title = routeTitles[path];
    } else if (path.startsWith('/category/')) {
      const cat = decodeURIComponent(path.replace('/category/', ''));
      document.title = `LuxeStore | ${cat} Collection`;
    } else if (path.startsWith('/products/')) {
      document.title = 'LuxeStore | Product Details';
    } else if (path.startsWith('/blog/')) {
      document.title = 'LuxeStore | Blog Article';
    } else {
      document.title = 'LuxeStore | 404 - Page Not Found';
    }
  }, [location]);

  return null;
}
