import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';

import { TOOLTIP_SHOW_DELAY } from '@/components/Grid/constants';
import { useOpenPopUpViaUrl } from '@/components/PopUpUrlInterceptor/hooks/useOpenPopUpViaUrl';
import { PopUpSearchParams } from '@/constants/searchParams';
import { getFullName } from '@/helpers/utils';
import { usePersonalInfoPopupGuard } from '@/hooks/usePersonalInfoPopupGuard';
import {
	EmployeeCellWrapper,
	EmployeeField,
	EmployeeFieldName,
	EmployeeHeaderRow,
} from '@/pages/Payload/styles';
import { EmployeeInfo, PayloadGridRow } from '@/pages/Payload/types';
import { formatSpecialization } from '@/pages/Payload/utils/formatSpecialization';
import { RenderCellPropsWithHoverIndex } from '@/types/columnHoverTypes';

export const employeeCellRenderer = ({
	row,
	column: { key },
}: RenderCellPropsWithHoverIndex<PayloadGridRow, unknown>) => {
	const canOpenProfile = usePersonalInfoPopupGuard();
	const handlePageOpen = useOpenPopUpViaUrl(PopUpSearchParams.UserIdParam);
	const info = row[key as keyof PayloadGridRow] as EmployeeInfo;
	const {
		firstName,
		lastName,
		firstNameRu,
		lastNameRu,
		specializationName,
		specializationSeniority,
		isActive,
	} = info ?? {
		firstName: '',
		lastName: '',
		firstNameRu: '',
		lastNameRu: '',
		specializationName: '',
		specializationSeniority: '',
		isActive: false,
	};

	const fullName = getFullName(firstName, lastName, firstNameRu, lastNameRu);
	const specialization = formatSpecialization(
		specializationName,
		specializationSeniority
	);

	const haveAccesToPopup =
		row.id !== undefined && isActive && canOpenProfile(+row.id);

	const handleClickName = () => {
		if (!haveAccesToPopup) {
			return;
		}
		handlePageOpen(row.id.toString());
	};

	const isClickAllowed = !row.disabled && isActive;

	return (
		<EmployeeCellWrapper disabled={row.disabled}>
			<EmployeeHeaderRow>
				<Tooltip
					useIfTextCut
					closeOnAnchorTouchEnd={isClickAllowed}
					showDelay={isClickAllowed ? TOOLTIP_SHOW_DELAY : undefined}
					textCutOrientation="horizontal"
					text={fullName}
				>
					<EmployeeFieldName
						isNeedToUnderlineUser={haveAccesToPopup}
						onClick={handleClickName}
					>
						{fullName}
					</EmployeeFieldName>
				</Tooltip>
			</EmployeeHeaderRow>

			<Tooltip
				useIfTextCut
				textCutOrientation="horizontal"
				text={specialization}
			>
				<EmployeeField isSpecializationField>{specialization}</EmployeeField>
			</Tooltip>
		</EmployeeCellWrapper>
	);
};
