function toggleChildCategories(element) {
    var childCategoryDiv = element.parentNode.querySelector('.child-category-list');

    if (childCategoryDiv) {
        // Alt kategori görünürlüğünü değiştir
        if (childCategoryDiv.style.display === 'none' || childCategoryDiv.style.display === '') {
            childCategoryDiv.style.display = 'block';
        } else {
            childCategoryDiv.style.display = 'none';
        }
    } else {
        console.error("Alt kategori div'i bulunamadı!");
    }
}
