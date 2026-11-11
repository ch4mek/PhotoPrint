import { Theme } from '@emotion/react';
import { JSX, ReactElement } from 'react';

import { Roles } from '@/constants/roles';
import { RoutesType } from '@/constants/routes';
import { UserClaims } from '@/types/claims';

export interface NavigationItem {
	id: number;
	label: string;
	icon: ReactElement | JSX.Element;
	payload: NavigationItemPayload;
}

export interface NavigationItemPayload {
	slug: RoutesType[keyof RoutesType];
	roles: Roles[];
	claims: UserClaims[];
}

export interface MainContentProps {
	withPaddings: boolean;
	theme?: Theme;
}

export interface LayoutProps {
	withPaddings?: boolean;
	withScroll?: boolean;
	logoTitle?: string;
	tabs?: NavigationItem[][];
}
