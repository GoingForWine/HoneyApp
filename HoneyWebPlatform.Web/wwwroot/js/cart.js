var connection = new signalR.HubConnectionBuilder().withUrl("/cartHub").build();

connection.on("CartItemQuantityUpdated", function (productId, quantity) {
    // Handle the client-side logic to update the UI based on the quantity change
    updateCartItemUI(productId, quantity);
    //console.log(`Received and executed the update for Product ID: ${productId}, Quantity: ${quantity}`);
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});

function updateCartItemQuantity(input) {
    var productId = input.getAttribute('data-product-id');
    var quantity = parseInt(input.value);

    // Call the server-side method to update the quantity in the database
    connection.invoke("UpdateCartItemQuantity", productId, quantity).catch(function (err) {
        return console.error(err.toString());
    });
    //console.log(`Updated quantity for Product ID: ${productId}, New Quantity: ${quantity}`);
}

function updateCartItemUI(productId, quantity) {
    //console.log(`Updating UI for Product ID: ${productId}, New Quantity: ${quantity}`);
    
    // Find the table row by product ID
    var tableRow = document.querySelector(`.product-row[data-product-id="${productId}"]`);

    if (!tableRow) {
        console.error(`Table row not found for Product ID: ${productId}`);
        return;
    }
    
    // Update the quantity input
    var quantityInput = tableRow.querySelector(`.product-quantity[data-product-id="${productId}"]`);
    if (!quantityInput) {
        console.error(`Quantity input not found for Product ID: ${productId}`);
        return;
    }

    quantityInput.value = quantity;

    // Update the total price cell
    var totalPriceCell = tableRow.querySelector(`.total-price[data-product-id="${productId}"]`);
    if (!totalPriceCell) {
        console.error(`Total price cell not found for Product ID: ${productId}`);
        return;
    }

    var pricePerItem = parseFloat(totalPriceCell.dataset.price);
    var totalPrice = pricePerItem * quantity;
    totalPriceCell.textContent = `${totalPrice.toFixed(2)} лева`;

    // Optionally, update the overall total price in the table header
    var cartTotal = document.querySelector('.cart-total');
    if (!cartTotal) {
        console.error('Cart total element not found');
        return;
    }

    // Recalculate the overall total price based on the updated quantities
    var updatedTotalPrice = calculateUpdatedTotalPrice();
    cartTotal.textContent = `${updatedTotalPrice.toFixed(2)} лева`;
    
    console.log(`Updated UI for Product ID: ${productId}, New Quantity: ${quantity}`);
}


function calculateUpdatedTotalPrice() {
    

    var totalPriceCells = document.querySelectorAll('.total-price');
    
    var updatedTotalPrice = 0;
    

    totalPriceCells.forEach(function (totalPriceCell) {
        
        var quantity = parseInt(totalPriceCell.previousElementSibling.querySelector('.product-quantity').value);
        
        var pricePerItem = parseFloat(totalPriceCell.dataset.price);
       
        updatedTotalPrice += pricePerItem * quantity;
       
    });
    

    return updatedTotalPrice;
}
