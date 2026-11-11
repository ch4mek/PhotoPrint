import { HEIGHT_CELL } from '../constants';

export function newCellHeight(isCellExpanded: boolean, groupsCount: number) {
	return isCellExpanded ? HEIGHT_CELL * groupsCount + HEIGHT_CELL : HEIGHT_CELL;
}
