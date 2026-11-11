import { GridCellDefaultProps } from '@/components/Grid';

export interface ClaimsRow extends GridCellDefaultProps {
	name: string;
	[key: string]: unknown;
}

export interface UserClaimsRow extends ClaimsRow {
	color: string;
}

export interface UserClaimsGroup extends ClaimsRow {
	name: string;
	claims: {
		name: string;
		value: string;
	}[];
}

export interface ClaimsRowConfig {
	name: string;
	width: number;
	key: string;
}

export interface User {
	userId: number;
	userFullName: string;
}
