import {
	createSearchParams,
	NavigateOptions,
	ParamKeyValuePair,
	useNavigate,
} from 'react-router-dom';

import { TAB_ID_PARAM, PopUpSearchParams } from '@/constants/searchParams';

interface TabQueryOptions {
	key: string;
	value: number;
}

interface NavigationOptions extends NavigateOptions {
	to?: string;
	tabId?: number | TabQueryOptions;
	userId?: number | string;
	params?: [string, string][];
}

export function useCustomNavigate() {
	const navigate = useNavigate();

	const handleNavigate = ({
		to,
		tabId,
		userId,
		params,
		...rest
	}: NavigationOptions) => {
		const searchParams: ParamKeyValuePair[] = [];

		if (tabId) {
			if (typeof tabId === 'number') {
				searchParams.push([TAB_ID_PARAM, tabId.toString()]);
			} else {
				const { key, value } = tabId;
				searchParams.push([key, value.toString()]);
			}
		}

		if (userId) {
			searchParams.push([PopUpSearchParams.UserIdParam, userId.toString()]);
		}

		if (params) {
			searchParams.push(...params);
		}

		navigate(
			{
				pathname: to,
				search: createSearchParams(searchParams).toString(),
			},
			{ ...rest }
		);
	};

	return handleNavigate;
}
