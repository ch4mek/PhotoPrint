import { useMemo } from 'react';

import { Loader } from '@/components/Loader';
import { departmentIdsWithCertificates } from '@/constants/departments';
import { isRegularEmployee } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';

import { CertificatesGrid } from './CertificatesGrid';
import { EducationsGrid } from './EducationsGrid';
import { useCertificatesGridState } from './hooks/useCertificatesGridState';
import { useEducationsGridState } from './hooks/useEducationsGridState';
import { EducationTabWrapper } from './styles';

export function EducationTab() {
	const { departmentId, position } = useAuth();

	const isCertificatesGridVisible = useMemo(
		() =>
			departmentIdsWithCertificates.includes(departmentId) &&
			isRegularEmployee(position),
		[departmentId]
	);

	const {
		rows: educationRows,
		columns: educationColumns,
		educationsPagination,
		handleSortColumnChange: handleEducationsSortChange,
		sortColumns: educationSortColumns,

		isLoading: isLoadingEducations,
	} = useEducationsGridState();

	const {
		rows: certificatesRows,
		columns: certificatesColumns,
		handleSortColumnChange: handleCertificatesSortChange,
		sortColumns: certificatesSortColumns,
		isDataLoading: isLoadingCertificates,
		userCertificatesGridPagination,
	} = useCertificatesGridState();

	return (
		<Loader
			isLoading={isLoadingEducations || isLoadingCertificates}
			size="small"
		>
			<EducationTabWrapper isHeightRestricted={isCertificatesGridVisible}>
				<EducationsGrid
					rows={educationRows}
					columns={educationColumns}
					pagination={educationsPagination}
					handleSortColumnChange={handleEducationsSortChange}
					sortColumns={educationSortColumns}
				/>
				{isCertificatesGridVisible && (
					<CertificatesGrid
						rows={certificatesRows}
						columns={certificatesColumns}
						handleSortColumnChange={handleCertificatesSortChange}
						sortColumns={certificatesSortColumns}
						pagination={userCertificatesGridPagination}
					/>
				)}
			</EducationTabWrapper>
		</Loader>
	);
}
