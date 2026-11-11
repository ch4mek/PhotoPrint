import { StatusEquipment } from '../constants';

export function checkBrokenStatus(status: string) {
	const brokenEquipmentStatuses = [
		StatusEquipment.BROKEN,
		StatusEquipment.RENOVATION,
		StatusEquipment.WRITTEN_OFF,
	];

	return brokenEquipmentStatuses.includes(status as StatusEquipment);
}
