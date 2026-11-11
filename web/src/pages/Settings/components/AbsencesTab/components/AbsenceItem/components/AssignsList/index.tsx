import { useTheme } from '@emotion/react';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { SearchInput } from '@mms/mms-ui-library/components/SearchInput';
import { useQueryClient } from '@tanstack/react-query';
import trim from 'lodash/trim';
import { ChangeEvent, useCallback, useEffect, useMemo, useState } from 'react';

import { NoDataFoundWrapper } from '@/components/NoDataFoundWrapper';
import { PopUpTitle } from '@/components/PopUpTitle';
import { NO_RESULTS_FOUND } from '@/constants/errors';
import { absencesQueriesKeys } from '@/constants/queriesKeys';
import { defaultSearchOptions } from '@/constants/search';
import { useToast } from '@/context/Toast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useDebounce } from '@/hooks/useDebounce';
import { useResize } from '@/hooks/useResize';
import { useGetAssigns } from '@/queries/absences/useGetAssigns';

import { UsersList } from './components/UsersList';
import {
	ASSIGNED_USERS_PER_PAGE,
	SEARCH_DEBOUNCE_DELAY,
	POPUP_TITLE,
} from './constants';
import { useMaxHeightLock } from './hooks/useMinHeightLock';
import { ContentWrapper } from './styles';
import { AssignsListProps } from './types';
import { getPopupStylesConfig } from './utils/getPopupStylesConfig';

const { maxCharacters } = defaultSearchOptions;

export function AssignsList({ id, onClose, entityName }: AssignsListProps) {
	const theme = useTheme();
	const toast = useToast();
	const queryClient = useQueryClient();
	const [, windowHeight] = useResize();
	const [search, setSearch] = useState('');

	const trimmedSearch = trim(search);
	const processedSearch =
		trimmedSearch.length > maxCharacters ? '' : trimmedSearch;

	const debouncedSearch = useDebounce(processedSearch, SEARCH_DEBOUNCE_DELAY);

	const { maxHeightLock, ref } = useMaxHeightLock<HTMLDivElement>();

	const { data, fetchNextPage, hasNextPage, error, isError, isLoading } =
		useGetAssigns(id, debouncedSearch, ASSIGNED_USERS_PER_PAGE, entityName);

	useEffect(
		() => () => {
			queryClient.removeQueries({
				queryKey: [absencesQueriesKeys.getAssigns, entityName, id],
				exact: false,
			});
		},
		[entityName, id, debouncedSearch]
	);

	useEffect(() => {
		if (isError) {
			createOnErrorHandler(toast)(error);
		}
	}, [toast, isError, error]);

	const handleNextPage = useCallback(() => {
		fetchNextPage();
	}, [fetchNextPage]);

	const handleSearchChange = useCallback((e: ChangeEvent<HTMLInputElement>) => {
		const value = e.target.value;
		setSearch(value);
	}, []);

	const handleResetSearch = useCallback(() => {
		setSearch('');
	}, []);

	const handleClose = useCallback(() => {
		queryClient.removeQueries({
			queryKey: [absencesQueriesKeys.getAssigns, entityName, id],
			exact: false,
		});
		onClose();
	}, [queryClient, entityName, id, onClose]);

	const popupStylesConfig = useMemo(
		() => getPopupStylesConfig(theme, windowHeight, maxHeightLock),
		[theme, windowHeight, maxHeightLock]
	);

	return (
		<PopUp
			title={<PopUpTitle headerText={POPUP_TITLE} />}
			onClose={handleClose}
			stylesConfig={popupStylesConfig}
			showLoader={isLoading}
		>
			<ContentWrapper ref={ref}>
				<SearchInput
					value={search}
					onChange={handleSearchChange}
					size="fullWidth"
					onDeleteIconClick={handleResetSearch}
				/>
				{data && (
					<NoDataFoundWrapper
						title={NO_RESULTS_FOUND}
						isEmptyData={data.length === 0}
					>
						<UsersList
							items={data}
							hasMoreItems={hasNextPage}
							onNextPage={handleNextPage}
						/>
					</NoDataFoundWrapper>
				)}
			</ContentWrapper>
		</PopUp>
	);
}
