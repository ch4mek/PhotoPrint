export const getRowKey = <T extends { id: string | number }>(row: T) => row.id;
