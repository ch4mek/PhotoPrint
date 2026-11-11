import { SingleTab } from '@mms/mms-ui-library/components/Tabs';
import { LazyExoticComponent, ReactElement } from 'react';

import { UserAccessClaims, UserClaims } from './claims';

export type ServerToastType = 'Info' | 'Success';

interface ToastReponse {
	toastType: ServerToastType;
	title: string;
	body: string;
}

export interface AutoselectOption<IdT = number> {
	id: IdT;
	name: string;
	disabled?: boolean;
}

export type ToastReponseWithBody = Omit<ToastReponse, 'title'>;

export type DecodedToken = UserAccessClaims & {
	aud?: string;
	exp?: number;
	iss?: string;
	nbf?: number;
};

export interface TabWithComponent extends SingleTab {
	Component: LazyExoticComponent<() => ReactElement | null>;
	claims?: UserClaims[];
}

export interface OdataParams<T> {
	top: number;
	skip: number;
	orderBy?: string;
	filters?: Partial<T>;
}

export interface ErrorResponseData {
	message?: string;
}

export type UnknownObject = Record<string, unknown>;
