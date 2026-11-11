import { JSX } from 'react';

export interface SectionEquipmentItemProps {
	name: string;
	icon: (props?: React.SVGProps<SVGSVGElement>) => JSX.Element;
	count: number;
}
