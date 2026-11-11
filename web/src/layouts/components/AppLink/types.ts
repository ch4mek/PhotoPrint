import { HTMLAttributes, JSX, ReactElement } from 'react';

import { Roles } from '@/constants/roles';

export interface AppLinkProps extends HTMLAttributes<HTMLAnchorElement> {
	title: string;
	href: string;
	icon: ReactElement | JSX.Element;
	roles: Roles[];
	hasSpecificColor: boolean;
}
