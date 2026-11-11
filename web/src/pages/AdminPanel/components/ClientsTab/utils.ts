import { ClientsGridRowType } from './constants';
import { ClientsGridRow } from './types';

export const isClientProjectsRow = (row: ClientsGridRow) =>
	row.type === ClientsGridRowType.PROJECT;
