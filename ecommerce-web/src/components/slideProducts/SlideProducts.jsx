import React from 'react'
import Product from './Product'
import './SlideProducts.css'
import { Swiper, SwiperSlide } from 'swiper/react';
import 'swiper/css';
import 'swiper/css/navigation';
import { Autoplay, Navigation } from 'swiper/modules';

export default function SlideProducts({ data, title }) {
  const getSubTitle = (title) => {
    switch (title?.toLowerCase()) {
      case 'smartphones': return 'Latest mobile technology at your fingertips.';
      case 'laptops': return 'Powerful machines for work and play.';
      default: return 'Explore our wide range of products with the best prices.';
    }
  };

  return (
    <div className='slide_products slide'>
      <div className='container'>
        <div className='top_slide'>
          <h2> {title} </h2>
          <p> {getSubTitle(title)} </p>
        </div>

        <Swiper 
          slidesPerView="auto"
          spaceBetween={15}
          loop={data?.length > 5}
          autoplay={{ delay: 2500, disableOnInteraction: false }}
          navigation={true}
          modules={[Navigation, Autoplay]}
          className="mySwiper"
        >
          {data?.map((item) => (
            <SwiperSlide key={item.id}> 
               <Product item={item} />
            </SwiperSlide>
          ))}
        </Swiper>
      </div>
    </div>
  )
}