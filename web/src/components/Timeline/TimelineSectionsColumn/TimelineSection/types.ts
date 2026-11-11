import { EquipmentItem, Section } from '../../types';

export interface TimelineSectionProps extends Section {
	isSelected: boolean;
	onSectionSelection?: (section: Section) => void;
	equipments: EquipmentItem[];
	countOfSeats: number;
}
