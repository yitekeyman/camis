export enum LoadActions {
  StartLoad = "STARTLOAD",
  StopLoad = "STOPLOAD"
}


export const LoadActionCreators = {
  start: () => ({ type : LoadActions.StartLoad}),
  stop : () => ({ type : LoadActions.StopLoad })
}