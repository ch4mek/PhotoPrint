import { GridCellDefaultProps } from '@/components/Grid';

export interface ContractExpirationGridRow extends GridCellDefaultProps {
	id: number;
	userId: number;
	user: string;
	dates: string;
}
