import React from 'react';

function ProductImages({ product }) {
    if (!product) return null;

    const mainImage = product.pictureUrl || product.images?.[0] || product.thumbnail || "/src/img/no-results.png";
    const allImages = product.images && product.images.length > 0 
        ? product.images 
        : [mainImage];

    return (
        <div className="imgs_items">
            <div className="big_img">
                <img id="big_img" src={mainImage} alt={product.name || product.title} />
            </div>

            {allImages.length > 1 && (
                <div className="sm_img">
                    {allImages.map((img, index) => (
                        <div key={index} className="img_div_sm">
                            <img
                                src={img}
                                alt={`${product.name || product.title} view ${index + 1}`}
                                onClick={() => {
                                    const el = document.getElementById("big_img");
                                    if (el) el.src = img;
                                }}
                            />
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
}

export default ProductImages;
