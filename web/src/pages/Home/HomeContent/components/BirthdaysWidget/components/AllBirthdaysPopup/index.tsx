import { useTheme } from '@emotion/react';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { SearchInput } from '@mms/mms-ui-library/components/SearchInput';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { ChangeEvent, useState, useMemo } from 'react';
import { useTranslation } from 'react-i18next';

import { MonthDropdown } from '../MonthDropdown';

import { Loader } from '@/components/Loader';
import { useToast } from '@/context/Toast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useDebounce } from '@/hooks/useDebounce';
import { useResize } from '@/hooks/useResize';
import { useGetAllBirthdays } from '@/queries/home/useGetAllBirthdays';

import { SEARCH_INPUT_DELAY } from './constants';
import { MonthsList, InputAndLabelWrapper } from './styles';
import { AllBirthdaysPopupProps } from './types';
import { getPopUpStylesConfig, countResults, getResultsString } from './utils';

export function AllBirthdaysPopup({ closePopup }: AllBirthdaysPopupProps) {
	const [windowWidth, windowHeight] = useResize();
	const theme = useTheme();
	const toast = useToast();
	const { t } = useTranslation('home', { keyPrefix: 'widgets.birthdays' });

	const [searchValue, setSearchValue] = useState('');
	const debouncedSearchValue = useDebounce(searchValue, SEARCH_INPUT_DELAY);

	const { data: birthdays, isLoading } = useGetAllBirthdays(
		debouncedSearchValue,
		{ onError: createOnErrorHandler(toast) }
	);

	const handleSearchChange = (e: ChangeEvent<HTMLInputElement>) => {
		setSearchValue(e.target.value);
	};

	const resetSearchValue = () => {
		setSearchValue('');
	};

	const popUpStylesConfig = useMemo(
		() =>
			getPopUpStylesConfig({
				theme,
				windowHeight,
				windowWidth,
			}),
		[theme, windowHeight, windowWidth]
	);

	const filteredBirthdaysCount = useMemo(
		() => countResults(birthdays, debouncedSearchValue),
		[birthdays, debouncedSearchValue]
	);

	return (
		<PopUp
			title={t('title')}
			onClose={closePopup}
			stylesConfig={popUpStylesConfig}
		>
			<InputAndLabelWrapper>
				<SearchInput
					value={searchValue}
					onChange={handleSearchChange}
					onDeleteIconClick={resetSearchValue}
				/>
				{filteredBirthdaysCount !== null && (
					<Typography variant="m-400" color={theme.palette['text-gray']}>
						{getResultsString(filteredBirthdaysCount, t)}
					</Typography>
				)}
			</InputAndLabelWrapper>
			<Loader {...{ isLoading }} size="small">
				<MonthsList>
					{birthdays?.map((birthday) => (
						<MonthDropdown key={birthday.month} {...birthday} />
					))}
				</MonthsList>
			</Loader>
		</PopUp>
	);
}
