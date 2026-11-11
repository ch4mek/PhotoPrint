import { Dispatch, SetStateAction } from 'react';

import { HistoryItem } from '../types';

import { EquipmentHistoryData } from './History/types';

export interface EquipmentUsersProps extends EquipmentHistoryData {
	isEdit: boolean;
	equipmentId: string;
	historyItems: Array<HistoryItem>;
	onLoad: Dispatch<SetStateAction<boolean>>;
	onClose: VoidFunction;
}
