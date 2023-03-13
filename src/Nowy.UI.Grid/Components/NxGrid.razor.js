export function table_size_update(wrapper, table, thead, tbody) {
    let height_wrapper = Number(String(window.getComputedStyle(wrapper, null).height).slice(0, -2));
    let height_table = Number(String(window.getComputedStyle(table, null).height).slice(0, -2));
    let height_thead = Number(String(window.getComputedStyle(thead, null).height).slice(0, -2));
    let height_tbody = Number(String(window.getComputedStyle(tbody, null).height).slice(0, -2));
    console.log('table_size_update', {
        height_wrapper: height_wrapper,
        height_table: height_table,
        height_thead: height_thead,
        height_tbody: height_tbody,
    });
    /*
    if (Math.abs(height_tbody - height_tbody_new) > 1) {
        tbody.style.height = `${height_tbody_new}px`;
    }
    */
    let offset_parent_element = wrapper.offsetParent;
    if (offset_parent_element) {
        table.style.height = `${window.innerHeight}px`;
        let offset_bottom = offset_parent_element.offsetHeight - wrapper.offsetTop - wrapper.offsetHeight;
        tbody.style.height = `${window.innerHeight - wrapper.offsetTop - offset_bottom - height_thead}px`;
        console.log('table_size_update', {
            'table.style.height': table.style.height,
            'tbody.style.height': tbody.style.height,
        });
    }
    return "0";
}
export function table_size_info(wrapper, table, thead, tbody) {
    let height_wrapper = Number(String(window.getComputedStyle(wrapper, null).height).slice(0, -2));
    let height_table = Number(String(window.getComputedStyle(table, null).height).slice(0, -2));
    let height_thead = Number(String(window.getComputedStyle(thead, null).height).slice(0, -2));
    let height_tbody = Number(String(window.getComputedStyle(tbody, null).height).slice(0, -2));
    console.log('table_size_info', {
        height_wrapper: height_wrapper,
        height_table: height_table,
        height_thead: height_thead,
        height_tbody: height_tbody,
    });
    return "0";
}
//# sourceMappingURL=NxGrid.razor.js.map