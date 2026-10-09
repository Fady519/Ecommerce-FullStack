import React, { useEffect, useState } from 'react'
import HeroSlider from '../../components/HeroSlider'
import './home.css'
import SlideProductLoading from '../../components/slideProducts/SlideProductLoading'
import PageTransition from '../../components/PageTransition'
// استيراد المكون الصحيح من مكانه
import SlideProducts from './../../components/slideProducts/SlideProducts';

const categories = [
  "smartphones",
  "mobile-accessories",
  "laptops",
  "tablets",
  "sunglasses"
]

// 🟢 التغيير هنا: غيرنا اسم الفانكشن لـ Home
export default function Home() {

  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchProducts = async () => {
      try {
        const result = await Promise.all(
          categories.map(async (category) => {
            const res = await fetch(`https://dummyjson.com/products/category/${category}`)
            const data = await res.json();
            return { [category]: data.products }
          })
        )

        const productsData = Object.assign({}, ...result);
        setProducts(productsData)
      }
      catch (error) {
        console.error("Error Fetching", error);
      } finally {
        setLoading(false);
      }
    }

    fetchProducts();
  }, []);

  return (
    <PageTransition>
      <div>
        <HeroSlider />

        {loading ? (
          categories.map((category) => (
            <SlideProductLoading key={category} />
          ))
        ) : (
          categories.map((category) => (
            /* 🟢 هنا الـ SlideProducts جاية من الـ components مش من نفس الملف */
            <SlideProducts 
               key={category} 
               data={products[category]} 
               title={category.replace("-", " ")} 
            />
          ))
        )}
      </div>
    </PageTransition>
  )
}