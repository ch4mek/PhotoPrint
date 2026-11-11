import {
	EVEN_SPECIALIZATION_ROW_CLASS_NAME,
	ODD_SPECIALIZATION_ROW_CLASS_NAME,
} from '@/pages/TeamBoardUser/components/SpecializationTable/constants';
import { SpecializationRow } from '@/pages/TeamBoardUser/types';

export const getRowKey = (row: SpecializationRow) => row.id;

export const getRowHeight = (row: SpecializationRow) => row.height || 0;

export const getRowClass = (row: SpecializationRow) =>
	row.isEven
		? EVEN_SPECIALIZATION_ROW_CLASS_NAME
		: ODD_SPECIALIZATION_ROW_CLASS_NAME;
