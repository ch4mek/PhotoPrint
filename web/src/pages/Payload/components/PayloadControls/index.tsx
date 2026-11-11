import { Button } from '@mms/mms-ui-library/components/Button';
import { FilterButton } from '@mms/mms-ui-library/components/FilterButton';
import { Popover } from '@mms/mms-ui-library/components/Popover';
import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { useOutsideClickMany } from '@mms/mms-ui-library/hooks/useOutsideClickMany';
import { useCallback, useRef, useState } from 'react';

import { PAYLOAD_MOBILE_BREAKPOINT } from '../../contants';
import { ShowTimeToggle } from '../ShowTimeToggle';

import { FilterPopoverContent } from '@/components/FilterPopoverContent';
import { ResetFilterButton } from '@/components/ResetFilterButton';
import { DEFAULT_POPOVER_OFFSET } from '@/constants/commonStyles';
import { controlsText } from '@/constants/controlsTexts';
import { usePayloadControlsFilter } from '@/pages/Payload/components/PayloadControls/hooks/usePayloadControlsFilter';
import { ControlBarWrapper } from '@/pages/Payload/components/PayloadControls/styles';
import { PayloadControlsBar } from '@/pages/Payload/components/PayloadControlsBar';

export function PayloadControls() {
	const [isFiltersPopoverOpen, setIsFiltersPopoverOpen] = useState(false);
	const isMobileView = !useHasScreenWidth(PAYLOAD_MOBILE_BREAKPOINT);

	const popoverRef = useRef(null);
	const filtersButtonRef = useRef(null);

	const { handleResetFilter, isResetFilterDisabled, ...filterProps } =
		usePayloadControlsFilter();

	const handleClosePopover = useCallback(() => {
		setIsFiltersPopoverOpen(false);
	}, []);

	const handleClickOnFilterButton = useCallback(() => {
		setIsFiltersPopoverOpen((prevValue) => !prevValue);
	}, []);

	const handleClearFilters = useCallback(() => {
		handleResetFilter();
		handleClosePopover();
	}, []);

	useOutsideClickMany([popoverRef, filtersButtonRef], handleClosePopover);

	return (
		<>
			{!isMobileView && (
				<ControlBarWrapper>
					<PayloadControlsBar isMobileView={isMobileView} {...filterProps} />
					<ResetFilterButton
						onClick={handleResetFilter}
						disabled={isResetFilterDisabled}
					/>
					<ShowTimeToggle />
				</ControlBarWrapper>
			)}

			{isMobileView && (
				<>
					<FilterButton
						size="big"
						isActive={!isResetFilterDisabled}
						isOpen={isFiltersPopoverOpen}
						onClick={handleClickOnFilterButton}
						ref={filtersButtonRef}
					/>

					<Popover
						open={isFiltersPopoverOpen}
						anchorElement={filtersButtonRef.current}
						position="bottom-end"
						ref={popoverRef}
						offset={DEFAULT_POPOVER_OFFSET}
					>
						<FilterPopoverContent>
							<PayloadControlsBar
								isMobileView={isMobileView}
								{...filterProps}
							/>
							<Button
								isFullWidth={false}
								color="secondary"
								onClick={handleClearFilters}
								disabled={isResetFilterDisabled}
							>
								{controlsText.resetText}
							</Button>
						</FilterPopoverContent>
					</Popover>
				</>
			)}
		</>
	);
}
