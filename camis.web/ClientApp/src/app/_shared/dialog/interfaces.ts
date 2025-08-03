export interface IDialogButton {
  name: string;
  className?: string;
  onClick?: Function;
  noCloseOnClick?: boolean;
}

export interface IDialog {
  title?: string;
  message: string;
  buttons?: IDialogButton[];
  titleClassName?: string;
  messageClassName?: string;
}
