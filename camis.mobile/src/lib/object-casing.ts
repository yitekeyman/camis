export function camelCase(obj: any): void {
  for (let key in obj) {
    if (obj.hasOwnProperty(key)) {
      key = key.toString()
      const newKey = key.substr(0, 1).toLowerCase() + key.substr(1)

      const oldVal = obj[key]
      delete obj[key]
      obj[newKey] = oldVal

      if (typeof obj[newKey] == "object") {
        camelCase(obj[newKey])
      }
    }
  }
}

export function PascalCase(obj: any): void {
  for (let key in obj) {
    if (obj.hasOwnProperty(key)) {
      key = key.toString()
      const newKey = key.substr(0, 1).toUpperCase() + key.substr(1)

      const oldVal = obj[key]
      delete obj[key]
      obj[newKey] = oldVal

      if (typeof obj[newKey] == "object") {
        PascalCase(obj[newKey])
      }
    }
  }
}
