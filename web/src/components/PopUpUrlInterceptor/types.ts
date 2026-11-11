import { LazyExoticComponent, JSX } from 'react';

import { Roles } from '@/constants/roles';
import { PopUpSearchParams } from '@/constants/searchParams';
import { UserClaims } from '@/types/claims';

interface PopUpProps {
	id: string;
	handleClose: VoidFunction;
}

export interface SearchParamsConfig
	extends Record<
		PopUpSearchParams,
		{
			PopUp: LazyExoticComponent<(props: PopUpProps) => JSX.Element>;
			isAuthRequired: boolean;
			roles: Roles[];
			claims: UserClaims[];
			requireNotOwnProfile?: boolean;
		}
	> {}
