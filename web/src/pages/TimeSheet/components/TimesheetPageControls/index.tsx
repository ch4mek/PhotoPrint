import { Button } from '@mms/mms-ui-library/components/Button';
import { FilterButton } from '@mms/mms-ui-library/components/FilterButton';
import { Popover } from '@mms/mms-ui-library/components/Popover';
import { useOutsideClickMany } from '@mms/mms-ui-library/hooks/useOutsideClickMany';
import { MouseEvent, useCallback, useMemo, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { TimeSheetBreakpoints } from '../../constants';
import { useIsPersonalModeOfPMO } from '../../hooks/useIsPersonalModeOfPMO';
import { JiraImportButton } from '../JiraImportButton';
import { TimeSheetControlsBar } from '../TimeSheetControlsBar';
import { TimesheetSubmitStatus } from '../TimesheetSubmitStatus';

import { PageHeader } from '@/components/PageHeader';
import { isRegularEmployee } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { useMobileView } from '@/hooks/useMobileView';

import {
	FiltersWrapper,
	PopoverContentWrapper,
	FiltersGroupWrapper,
	FiltersViewWrapper,
	ActionControlsWrapper,
} from './styles';
import { TimesheetPageControlsProps } from './types';

const { DESKTOP_MD, TABLET } = TimeSheetBreakpoints;

const popoverOffset: [number, number] = [0, 16];

export function TimesheetPageControls(props: TimesheetPageControlsProps) {
	const {
		clearAllFilter,
		isResetDisabled,
		engineerStatus = null,
		shouldRenderSubmitStatusForEmployee,
		isEmployeeOrSysAdminRole,
	} = props;
	const [isFiltersPopoverOpen, setIsFiltersPopoverOpen] = useState(false);
	const { role } = useAuth();
	const popoverRef = useRef(null);
	const showFiltersButtonRef = useRef(null);
	const isMobileView = useMobileView(DESKTOP_MD);
	const isTabletView = useMobileView(TABLET);
	const { t: tCommon } = useTranslation('common');
	const { t } = useTranslation('timesheet');

	const isPersonalModeOfPMO = useIsPersonalModeOfPMO();
	const showJiraInFilterGroup =
		!isPersonalModeOfPMO && !shouldRenderSubmitStatusForEmployee;
	const showActionControls =
		isPersonalModeOfPMO || shouldRenderSubmitStatusForEmployee;
	const filtersViewProps = {
		isPersonalModeOfPMO,
		shouldRenderSubmitStatusForEmployee,
	};

	const toggleFiltersPopupOpen = useCallback(() => {
		setIsFiltersPopoverOpen(
			(prevAreFiltersPopupOpen) => !prevAreFiltersPopupOpen
		);
	}, []);

	const handleClickOutside = useCallback(() => {
		setIsFiltersPopoverOpen(false);
	}, []);

	const handleOnClickPopoverContent = useCallback(
		(e: MouseEvent) => e.stopPropagation(),
		[]
	);

	const handlerOnClickClearButton = useMemo(
		() => (!isResetDisabled ? clearAllFilter : undefined),
		[isResetDisabled]
	);

	useOutsideClickMany([popoverRef, showFiltersButtonRef], handleClickOutside);

	const isEmployeeRole = useMemo(() => isRegularEmployee(role), [role]);

	const fillAvailableWidthOnMobile = isMobileView && isEmployeeOrSysAdminRole;

	return (
		<FiltersWrapper isSubmitRendered={shouldRenderSubmitStatusForEmployee}>
			{isMobileView && !isEmployeeOrSysAdminRole && (
				<PageHeader title={t('title')} />
			)}

			{(!isMobileView || isEmployeeOrSysAdminRole) && (
				<FiltersViewWrapper {...filtersViewProps}>
					<FiltersGroupWrapper fillAvailableWidth={fillAvailableWidthOnMobile}>
						<TimeSheetControlsBar
							{...props}
							onClick={handlerOnClickClearButton}
							disabled={isResetDisabled}
						/>
					</FiltersGroupWrapper>
					<ActionControlsWrapper>
						<JiraImportButton />
						{shouldRenderSubmitStatusForEmployee && !isTabletView && (
							<TimesheetSubmitStatus status={engineerStatus} />
						)}
					</ActionControlsWrapper>
				</FiltersViewWrapper>
			)}

			{isMobileView && !isEmployeeOrSysAdminRole && (
				<FiltersViewWrapper {...filtersViewProps}>
					<FiltersGroupWrapper fillAvailableWidth={fillAvailableWidthOnMobile}>
						{showJiraInFilterGroup && <JiraImportButton />}
						<FilterButton
							size="big"
							isActive={!isResetDisabled}
							isOpen={isFiltersPopoverOpen}
							onClick={toggleFiltersPopupOpen}
							ref={showFiltersButtonRef}
						/>
						<Popover
							open={isFiltersPopoverOpen}
							anchorElement={showFiltersButtonRef.current}
							position="bottom-end"
							ref={popoverRef}
							offset={popoverOffset}
						>
							<PopoverContentWrapper
								isEmployee={isEmployeeRole}
								onClick={handleOnClickPopoverContent}
							>
								<TimeSheetControlsBar {...props} />
								<Button
									isFullWidth={false}
									color="secondary"
									onClick={handlerOnClickClearButton}
									disabled={isResetDisabled}
								>
									{tCommon('controlButtons.reset')}
								</Button>
							</PopoverContentWrapper>
						</Popover>
					</FiltersGroupWrapper>
					{showActionControls && (
						<ActionControlsWrapper>
							{isPersonalModeOfPMO && <JiraImportButton />}
							{shouldRenderSubmitStatusForEmployee && (
								<TimesheetSubmitStatus status={engineerStatus} />
							)}
						</ActionControlsWrapper>
					)}
				</FiltersViewWrapper>
			)}
		</FiltersWrapper>
	);
}
