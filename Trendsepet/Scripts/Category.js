// Firestore ile bağlantı ayarları
const projectId = 'trendsepet-c6453'; // Proje ID'nizi buraya yazın

// Ana Kategorileri Getir
function loadCategories() {
    const url = `https://firestore.googleapis.com/v1/projects/${projectId}/databases/(default)/documents/categories_parent`;

    fetch(url, {
        method: 'GET',
        headers: {
            // Erişim token'ı kullanılmadığı için bu kısmı kaldırıyoruz
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            if (data.documents) {
                data.documents.forEach(doc => {
                    let parentCategory = doc.fields;

                    // Belge adını (ID olarak kullanmak için) al
                    let categoryId = doc.name.split('/').pop(); // 'name' alanından ID'yi alıyoruz
                    let dropdownItem = `<li><a href="Products.aspx?parentCategoryId=${categoryId}">${parentCategory.name.stringValue}</a></li>`;

                    $('#categoryDropdown').append(dropdownItem);
                });
            } else {
                console.log("Kategori bulunamadı.");
            }
        })
        .catch(error => {
            console.error("Kategoriler yüklenirken hata oluştu: ", error);
        });
}

// Sayfa yüklendiğinde kategorileri çek
$(document).ready(function () {
    loadCategories();
});
