import { useTranslation } from 'react-i18next';

import { InfoDataContainer } from '@/components/InfoDataContainer';
import { Item } from '@/components/InfoDataContainer/Item';
import {
	ContainerWrapper,
	ContentColumn,
	ContentEditWrapper,
} from '@/components/InfoDataContainer/styles';
import { DATE_FORMAT } from '@/constants/date';
import { getDateFormat } from '@/helpers/getDateFormat';
import { EmploymentHistoryResponse } from '@/types/employmentHistory';

import { EmploymentHistoryProps } from './types';

export function EmploymentHistory({
	employmentHistory,
}: EmploymentHistoryProps) {
	const {
		professionalActivityStartDate,
		firstProjectStartDate,
		hireDate,
		contractEndDate,
	} = employmentHistory || ({} as EmploymentHistoryResponse);

	const { t } = useTranslation('profile', { keyPrefix: 'employmentHistory' });

	return (
		<InfoDataContainer headerTitle={t('title')}>
			<ContainerWrapper>
				<ContentEditWrapper>
					<ContentColumn>
						<Item
							nameOfField={t('professionalActivityStartDate')}
							value={getDateFormat(professionalActivityStartDate, DATE_FORMAT)}
						/>
						<Item
							nameOfField={t('hireDate')}
							value={getDateFormat(hireDate, DATE_FORMAT)}
						/>
					</ContentColumn>
					<ContentColumn>
						<Item
							nameOfField={t('firstProjectStartDate')}
							value={getDateFormat(firstProjectStartDate, DATE_FORMAT)}
						/>
						<Item
							nameOfField={t('contractEndDate')}
							value={getDateFormat(contractEndDate, DATE_FORMAT)}
						/>
					</ContentColumn>
				</ContentEditWrapper>
			</ContainerWrapper>
		</InfoDataContainer>
	);
}
