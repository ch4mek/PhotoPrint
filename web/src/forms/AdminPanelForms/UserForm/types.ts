import { Theme } from '@emotion/react';
import { UseMutateAsyncFunction } from '@tanstack/react-query';
import { AxiosError } from 'axios';

import type { UserFormValues } from '@/forms/types';
import { ServerResponseError } from '@/queries/types';
import { UseUpdateUserParam, UseUpdateUserResponse } from '@/types/userInfo';

import { UserFormTabs } from './constants';

export interface UserFormProps {
	initialValues: UserFormValues;
	handleClose: () => void;
	onSubmit: UseMutateAsyncFunction<
		UseUpdateUserResponse,
		AxiosError<ServerResponseError, any>,
		UseUpdateUserParam,
		unknown
	>;
	id: number;
	isEmailDuplicate: boolean;
	onEmailChange: () => void;
	hasHRModuleClaim: boolean;
}

export interface GetPopUpStylesConfigProps {
	theme: Theme;
	hasTabs: boolean;
	activeTab: UserFormTabs;
	windowHeight: number;
	windowWidth: number;
}

export interface GetTabWrapperHeightProps {
	theme: Theme;
	activeTab: UserFormTabs;
	windowHeight: number;
	windowWidth: number;
	userTabHeight: number;
}
