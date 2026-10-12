import React from 'react';
import { Swiper, SwiperSlide } from 'swiper/react';
import 'swiper/css';
import 'swiper/css/pagination';

import banner1 from '../../src/img/banner_Hero1.jpg';
import banner2 from '../../src/img/banner_Hero2.jpg';
import banner3 from '../../src/img/banner_Hero3.jpg';

import { Autoplay, Pagination } from 'swiper/modules';
import { Link } from 'react-router-dom';

function HeroSlider() {
    return (
        <div className='container' style={{ marginTop: '20px' }}>
            <Swiper 
                loop={true} 
                autoplay={{ delay: 3500, disableOnInteraction: false }} 
                pagination={{ clickable: true }} 
                modules={[Pagination, Autoplay]} 
                className="mySwiper"
                style={{ borderRadius: '16px', overflow: 'hidden' }}
            >
                <SwiperSlide>
                    <div className='content'>
                        <h4>New Season Collection</h4>
                        <h3>Original Styles <br /> & Premium Wear</h3>
                        <p>Explore high quality tops, hoodies, and jackets from top global brands.</p>
                        <Link to="/category/all" className='btn'>Shop Collection</Link>
                    </div>
                    <img src={banner1} alt='slider hero 1' />
                </SwiperSlide>

                <SwiperSlide>
                    <div className='content'>
                        <h4>Exclusive Selection</h4>
                        <h3>Trendy Outerwear <br /> & Blazers</h3>
                        <p>Elegance meets comfort with our latest formal and casual selections.</p>
                        <Link to="/category/Outerwear%20%26%20Dresses" className='btn'>Explore Outerwear</Link>
                    </div>
                    <img src={banner2} alt='slider hero 2' />
                </SwiperSlide>

                <SwiperSlide>
                    <div className='content'>
                        <h4>Special Offers</h4>
                        <h3>Everyday Bottoms <br /> & Denim Jeans</h3>
                        <p>Discover durable stretch denim and casual trousers at unbeatable prices.</p>
                        <Link to="/category/Bottoms" className='btn'>Get Deals</Link>
                    </div>
                    <img src={banner3} alt='slider hero 3' />
                </SwiperSlide>
            </Swiper>
        </div>
    );
}

export default HeroSlider;
