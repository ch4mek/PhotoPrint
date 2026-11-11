import { Dispatch, SetStateAction } from 'react';

import { StatusEquipment } from '../../constants';

import { GridCellDefaultProps } from '@/components/Grid';
import { HistoryItem } from '@/forms/EquipmentForm/types';
import { FullName } from '@/helpers/formatName';

export interface EquipmentHistoryData {
	equipmentId: string;
	setHistoryItems: Dispatch<SetStateAction<Array<HistoryItem>>>;
	status: StatusEquipment;
}

export interface EquipmentHistoryProps extends EquipmentHistoryData {
	resetForm: () => void;
}

export interface EquipmentHistoryRow extends GridCellDefaultProps {
	userName: FullName;
	assignmentApprover: FullName;
	issueDate: string;
	returnDate: string;
	departmentId: string;
	id: string;
	equipmentHistoryId: number;
}
