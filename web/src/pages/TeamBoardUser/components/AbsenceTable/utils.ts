import { AbsenceRow } from '@/types/absences';

export const getRowKey = (row: AbsenceRow) => row.id ?? row.name;
