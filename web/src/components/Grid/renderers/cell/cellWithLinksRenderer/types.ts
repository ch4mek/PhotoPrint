import { DynamicHeightCellDefaultProps } from '@/components/Grid/renderers/types';

export interface CellWithLinksProps extends DynamicHeightCellDefaultProps {
	links: string[];
}
