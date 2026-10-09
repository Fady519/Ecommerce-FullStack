import { useEffect, useState } from "react";
import { useParams } from "react-router-dom"


import SlideProduct from "../../../components/slideProducts/SlideProducts";


import './ProductDetails.css'

import ProductDetailsLoading from "./ProductDetailsLoading";
import SlideProductLoading from "../../../components/slideProducts/SlideProductLoading";
import ProductImages from "./ProductImages";
import ProductInfo from "./ProductInfo";
import PageTransition from "../../../components/PageTransition";

function ProductDeatails() {

    const { id } = useParams();

    const [product, setProduct] = useState(null);

    const [loading, setLoading] = useState(true);

    const [relatedProducts, setRelatedProducts] = useState([]);

    const [loadingRelatedProducts, setLoadingRelatedProducts] = useState(true);

    useEffect(() => {
        const fetchProduct = async () => {
            try {
                const res = await fetch(`https://dummyjson.com/products/${id}`)
                const data = await res.json();
                setProduct(data);
                setLoading(false);


            } catch (erroe) {
                console.log(erroe);
            }
        }
        fetchProduct();
    }, [id]);



    useEffect(() => {
        if (!product) return
        fetch(`https://dummyjson.com/products/category/${product.category}`)
            .then((res) => res.json())
            .then((data) => {
                setRelatedProducts(data.products)
            })
            .catch((error) => console.log(error))
            .finally(() => setLoadingRelatedProducts(false))
    }, [product?.category]);



    if (!product) return <p> Product Not Found </p>
    return (

        <PageTransition key={id}>
            <div>

                {loading ? (
                    < ProductDetailsLoading />
                ) : (

                    <div className="item_details">

                        <div className="container">

                            < ProductImages product={product} />

                            < ProductInfo product={product} />

                        </div>

                    </div>
                )}

                {loadingRelatedProducts ? (
                    <SlideProductLoading />
                ) : (
                    <SlideProduct
                        key={product.category}
                        data={relatedProducts}
                        title={product.category.replace("_", " ")}
                    />
                )}


            </div>
        </PageTransition>

    );
}

export default ProductDeatails
