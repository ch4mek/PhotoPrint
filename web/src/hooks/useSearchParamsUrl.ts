import { useCallback } from 'react';
import { useSearchParams } from 'react-router-dom';

export const useSearchParamsUrl = () => {
	const [searchParams, setSearchParams] = useSearchParams();

	const setSearchParamsUrl = useCallback(
		(key: string, value: string, keysToDelete?: string[]) => {
			const newParams = new URLSearchParams(window.location.search);
			newParams.set(key, value);

			keysToDelete?.forEach((k) => newParams.delete(k));

			setSearchParams(newParams);
		},
		[setSearchParams]
	);

	const deleteSearchParamsUrl = useCallback(
		(...keys: string[]) => {
			const newParams = new URLSearchParams(window.location.search);
			keys.forEach((k) => newParams.delete(k));

			setSearchParams(newParams);
		},
		[setSearchParams]
	);

	return {
		setSearchParamsUrl,
		deleteSearchParamsUrl,
		searchParams,
	};
};
