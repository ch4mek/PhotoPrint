import { Absence } from '@/types/absences';

export const checkIsNameExist = (
	items: Absence[],
	name: string,
	id?: string | number
): boolean =>
	items.some((item) => item.name.trim() === name.trim() && item.id !== id);
