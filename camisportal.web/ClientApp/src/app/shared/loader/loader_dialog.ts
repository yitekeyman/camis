import swal, {SweetAlertOptions} from "sweetalert2";

const dialog = swal as typeof swal & {
    loading: (overrideOptions?: SweetAlertOptions) => void
};

dialog.loading = (overrideOptions?: SweetAlertOptions): void => {
    swal(Object.assign({
        allowOutsideClick: false,
    }, overrideOptions));
    swal.disableButtons();
    return swal.showLoading();
};

export default dialog;