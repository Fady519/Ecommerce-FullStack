import React, { useRef } from 'react';
import Product from './Product';
import './slideProducts.css';
import { Swiper, SwiperSlide } from 'swiper/react';
import 'swiper/css';
import { Autoplay } from 'swiper/modules';
import { Link } from 'react-router-dom';
import { HiArrowRight, HiChevronLeft, HiChevronRight } from "react-icons/hi2";

export default function SlideProducts({ data, title }) {
  const swiperRef = useRef(null);

  const getSubTitle = (categoryName) => {
    switch (categoryName?.toLowerCase()) {
      case 'tops': return 'Premium t-shirts, hoodies, and sweaters.';
      case 'bottoms': return 'Tailored trousers, casual pants, and denim jeans.';
      case 'outerwear & dresses': return 'Blazers, jackets, and seasonal outfits.';
      case 'accessories & footwear': return 'Original shoes, sneakers, and fashion accessories.';
      default: return `Explore our verified collection of ${categoryName}.`;
    }
  };

  return (
    <div className='slide_products slide'>
      <div className='container'>
        <div className='category_shelf_wrapper'>
          <div className='top_slide'>
            <div className='top_slide_info'>
              <h2>{title}</h2>
              <p>{getSubTitle(title)}</p>
            </div>

            <div className='top_slide_actions'>
              <Link 
                to={`/category/${encodeURIComponent(title)}`} 
                className='slide_view_all_link'
              >
                <span>View All ({data?.length || 0})</span>
                <HiArrowRight />
              </Link>

              <div className='slide_header_nav_btns'>
                <button 
                  type="button" 
                  className='nav_circle_btn' 
                  onClick={() => swiperRef.current?.slidePrev()} 
                  title="Previous products"
                  aria-label="Previous products"
                >
                  <HiChevronLeft />
                </button>
                <button 
                  type="button" 
                  className='nav_circle_btn' 
                  onClick={() => swiperRef.current?.slideNext()} 
                  title="Next products"
                  aria-label="Next products"
                >
                  <HiChevronRight />
                </button>
              </div>
            </div>
          </div>

          <div className='slider_outer_wrapper'>
            <button 
              type="button"
              className='slider_flank_btn flank_prev' 
              onClick={() => swiperRef.current?.slidePrev()}
              aria-label="Previous slide"
              title="Previous"
            >
              <HiChevronLeft />
            </button>

            <Swiper 
              onSwiper={(swiper) => { swiperRef.current = swiper; }}
              slidesPerView="auto"
              spaceBetween={18}
              loop={data?.length > 4}
              autoplay={{ delay: 3500, disableOnInteraction: false }}
              modules={[Autoplay]}
              className="mySwiper"
            >
              {data?.map((item) => (
                <SwiperSlide key={item.id} style={{ width: '250px' }}> 
                   <Product item={item} />
                </SwiperSlide>
              ))}
            </Swiper>

            <button 
              type="button"
              className='slider_flank_btn flank_next' 
              onClick={() => swiperRef.current?.slideNext()}
              aria-label="Next slide"
              title="Next"
            >
              <HiChevronRight />
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}