import { SectorProps } from 'recharts';

export interface ActiveShapeProps extends SectorProps {
	payload?: Record<string, number | string>;
	setName: (name: string) => void;
	setTime: (time: string) => void;
}
