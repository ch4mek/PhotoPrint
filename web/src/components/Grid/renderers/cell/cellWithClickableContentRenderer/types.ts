import { GridCellDefaultProps } from '../../types';

export interface CellWithSerialNumberProps extends GridCellDefaultProps {
	serialNumber: string;
	equipmentId: string;
}
