import React from 'react'

function SlideProductLoading() {
    return (
        <div className='loading_slide_products'>

            <div className='slide_products slide'>
                <div className='container'>
                    <div className='category_shelf_wrapper'>
                        <div className='top_slide'>
                            <h2 className='skeltion' style={{ width: '180px', height: '24px' }}></h2>
                            <p className='skeltion' style={{ width: '280px', height: '14px', marginTop: '6px' }}></p>
                        </div>

                        <div className='products_loading'>
                            <div className='product'>
                                <div className='img_product skeltion'></div>
                                <div className='content skeltion'></div>
                                <div className='content skeltion'></div>
                            </div>
                            <div className='product'>
                                <div className='img_product skeltion'></div>
                                <div className='content skeltion'></div>
                                <div className='content skeltion'></div>
                            </div>
                            <div className='product'>
                                <div className='img_product skeltion'></div>
                                <div className='content skeltion'></div>
                                <div className='content skeltion'></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

        </div>
    )
}

export default SlideProductLoading
