import swal, { SweetAlertOptions, SweetAlertResult } from 'sweetalert2';


const dialog = swal as typeof swal & {
  confirm: (question: string, title?: string, overrideOptions?: SweetAlertOptions) => Promise<boolean | null>
  confirmResult: (question: string, title?: string, overrideOptions?: SweetAlertOptions) => Promise<SweetAlertResult>
  error: (e: any, overrideOptions?: SweetAlertOptions) => Promise<SweetAlertResult>
  info: (message: string, title?: string, overrideOptions?: SweetAlertOptions) => Promise<SweetAlertResult>
  loading: (overrideOptions?: SweetAlertOptions) => void
  prompt: (question: string, title?: string, overrideOptions?: SweetAlertOptions) => Promise<string | null>
  promptResult: (question: string, title?: string, overrideOptions?: SweetAlertOptions) => Promise<SweetAlertResult>
  success: (message?: string, title?: string, overrideOptions?: SweetAlertOptions) => Promise<SweetAlertResult>
  warning: (e: any, overrideOptions?: SweetAlertOptions) => Promise<SweetAlertResult>
};


dialog.confirm = async (question: string, title?: string, overrideOptions?: SweetAlertOptions): Promise<boolean | null> => {
  const result = await dialog.confirmResult(question, title, overrideOptions);
  return result.dismiss && result.dismiss.toString().toLowerCase() == 'close' ? null : !!result.value;
};

dialog.confirmResult = async (question: string, title?: string, overrideOptions?: SweetAlertOptions): Promise<SweetAlertResult> => {
  return swal(Object.assign({
    type: 'question',
    confirmButtonText: 'Yes',
    cancelButtonText: 'No',
    title,
    text: question,
    showCloseButton: true,
    showConfirmButton: true,
    showCancelButton: true,
    allowOutsideClick: false,
  } as SweetAlertOptions, overrideOptions));
};

dialog.error = async (e: any, overrideOptions?: SweetAlertOptions): Promise<SweetAlertResult> => {
  return swal(Object.assign({
    type: 'error',
    title: 'Error!',
    text: e && (e.message || (typeof e === 'string' ? e : JSON.stringify(e))) || 'Unknown error.',
    showCloseButton: true,
    showConfirmButton: true,
    showCancelButton: false,
    allowOutsideClick: false,
  } as SweetAlertOptions, overrideOptions));
};

dialog.info = async (message: string, title?: string, overrideOptions?: SweetAlertOptions): Promise<SweetAlertResult> => {
  return swal(Object.assign({
    type: 'info',
    title,
    text: message,
    showCloseButton: true,
    showConfirmButton: true,
    showCancelButton: false,
    allowOutsideClick: true,
  } as SweetAlertOptions, overrideOptions));
};

dialog.loading = (overrideOptions?: SweetAlertOptions): void => {
  swal(Object.assign({
    allowOutsideClick: false,
    showCloseButton: false,
    showConfirmButton: false,
    showCancelButton: false,
  } as SweetAlertOptions, overrideOptions));
  swal.disableButtons();
  return swal.showLoading();
};

dialog.prompt = async (question: string, title?: string, overrideOptions?: SweetAlertOptions): Promise<string | null> => {
  const result = await dialog.promptResult(question, title, overrideOptions);
  return result.dismiss && result.dismiss.toString().toLowerCase() == 'close' ? null : result.value;
};

dialog.promptResult = async (question: string, title?: string, overrideOptions?: SweetAlertOptions): Promise<SweetAlertResult> => {
  return swal(Object.assign({
    type: 'question',
    input: 'text',
    title,
    text: question,
    showCloseButton: true,
    showConfirmButton: true,
    showCancelButton: false,
    allowOutsideClick: false,
  } as SweetAlertOptions, overrideOptions));
};

dialog.success = async (message?: string, title = 'Success!', overrideOptions?: SweetAlertOptions): Promise<SweetAlertResult> => {
  return swal(Object.assign({
    type: 'success',
    title,
    text: message,
    showCloseButton: true,
    showConfirmButton: true,
    showCancelButton: false,
    allowOutsideClick: true,
  } as SweetAlertOptions, overrideOptions));
};

dialog.warning = async (e: any, overrideOptions?: SweetAlertOptions): Promise<SweetAlertResult> => {
  return swal(Object.assign({
    type: 'warning',
    title: 'Warning!',
    text: e && (e.message || (typeof e === 'string' ? e : JSON.stringify(e))) || 'Unknown warning.',
    showCloseButton: true,
    showConfirmButton: true,
    showCancelButton: false,
    allowOutsideClick: false,
  } as SweetAlertOptions, overrideOptions));
};


export default dialog;
