import { SearchInput } from '@mms/mms-ui-library/components/SearchInput';
import { ChangeEvent, useCallback } from 'react';

import { ButtonAdd } from '@/components/ButtonAdd';
import { ExportControl } from '@/components/ExportControl';
import { DEFAULT_SEARCH_FIELD_PLACEHOLDER } from '@/constants/controlsTexts';
import { DISCOUNT_MESSAGES, NotificationType } from '@/constants/notifications';
import { useAuth } from '@/context/AuthContext';
import { useDiscountContext } from '@/context/DiscountContext';
import { useToast } from '@/context/Toast';
import { DiscountForm } from '@/forms/Discount/DiscountForm';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { createToast } from '@/helpers/createToast';
import { SearchPanelWrapper } from '@/pages/Discounts/components/DiscountsSection/components/DiscountControls/components/SearchPanel/styles';
import { SearchPanelProps } from '@/pages/Discounts/components/DiscountsSection/components/DiscountControls/components/SearchPanel/types';
import { generateDiscountsPDF } from '@/pages/Discounts/components/DiscountsSection/components/DiscountControls/components/SearchPanel/utils';
import { SearchInputWrapper } from '@/pages/Discounts/components/DiscountsSection/components/DiscountControls/styles';
import { useAddDiscountForm } from '@/pages/Discounts/components/DiscountsSection/hooks/useAddDiscountForm';
import { useGetDiscounts } from '@/queries/discounts/useGetDiscounts';
import { UserClaims } from '@/types/claims';

import { emptyDiscountsFilters, exportOptions } from './constants';

export function SearchPanel({ isControlsDisabled }: SearchPanelProps) {
	const { claims } = useAuth();

	const { searchValue, setSearchValue, isMoveModeEnable } =
		useDiscountContext();

	const {
		handleCreateDiscountFormOpen,
		handleCreateDiscountFormClose,
		isCreateDiscountFormOpen,
		handleAddDiscountFormSubmit,
	} = useAddDiscountForm();

	const handleSearchChange = useCallback((e: ChangeEvent<HTMLInputElement>) => {
		setSearchValue(e.target.value);
	}, []);

	const handleSearchReset = useCallback(() => {
		setSearchValue('');
	}, []);

	const toast = useToast();

	const { refetch: fetchDiscounts, isFetching } = useGetDiscounts(
		emptyDiscountsFilters,
		{
			enabled: false,
			onError: () => {
				toast.open(
					createToast(NotificationType.ERROR, DISCOUNT_MESSAGES.EXPORT_ERROR)
				);
			},
		}
	);

	const exportDiscountsToPDF = async () => {
		const { data: discounts, isError } = await fetchDiscounts();

		if (isError) {
			return;
		}

		if (discounts && discounts.length > 0) {
			generateDiscountsPDF(discounts);
		}
	};

	const isExportEnabled = checkIsHasClaims(
		[UserClaims.ManageDiscountsExport],
		claims
	);

	return (
		<SearchPanelWrapper>
			<SearchInputWrapper>
				<SearchInput
					placeholder={DEFAULT_SEARCH_FIELD_PLACEHOLDER}
					value={searchValue}
					onChange={handleSearchChange}
					size="fullWidth"
					disabled={isMoveModeEnable}
					onDeleteIconClick={handleSearchReset}
					showToolTipIfCut={false}
				/>
			</SearchInputWrapper>
			{isExportEnabled && (
				<ExportControl
					options={exportOptions}
					onChange={exportDiscountsToPDF}
					isDisabled={isFetching}
				/>
			)}
			{checkIsHasClaims([UserClaims.ManageDiscountCard], claims) && (
				<ButtonAdd
					disabled={isControlsDisabled || isMoveModeEnable}
					onAdd={handleCreateDiscountFormOpen}
				/>
			)}
			{isCreateDiscountFormOpen && (
				<DiscountForm
					handleClose={handleCreateDiscountFormClose}
					handleSubmit={handleAddDiscountFormSubmit}
				/>
			)}
		</SearchPanelWrapper>
	);
}
