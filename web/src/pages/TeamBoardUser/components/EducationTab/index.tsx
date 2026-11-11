import { useContext, useMemo } from 'react';

import { PersonalDataContext } from '../../context/personalDataContext';
import { isFullAccess } from '../../utils';

import { Loader } from '@/components/Loader';
import { departmentIdsWithCertificates } from '@/constants/departments';
import { isRegularEmployee } from '@/constants/roles';
import { useConfiguration } from '@/context/Configuration';
import { CertificateForm } from '@/forms/CertificateForm';
import { EducationForm } from '@/forms/TeamboardForm/Education';

import { CertificatesGrid } from './CertificatesGrid';
import { EducationsGrid } from './EducationsGrid';
import { useCertificatesGridState } from './hooks/useCertificatesGridState';
import { useEducationsGridState } from './hooks/useEducationsGridState';
import { EducationTabWrapper } from './styles';
import { getCertificateFormValuesFromPersonalData } from './utils/getCertificateFormValuesFromPersonalData';

export function EducationTab() {
	const {
		rows: educationRows,
		columns: educationColumns,
		educationsPagination,
		handleSortColumnChange: handleEducationsSortChange,
		sortColumns: educationSortColumns,
		isLoading: isLoadingEducations,
		handlePopupToggle: handleEducationFormToggle,
		isFormVisible: isEducationFormVisible,
		chosenEducation,
	} = useEducationsGridState();

	const {
		accessLevel,
		data,
		position,
		userId,
		isLoading: isLoadingPersonalData,
	} = useContext(PersonalDataContext);
	const {
		configurableValues: { departmentTypes, roleTypes },
	} = useConfiguration();

	const isCertificatesGridVisible = useMemo(
		() =>
			position && data.departmentId
				? isRegularEmployee(position) &&
				  departmentIdsWithCertificates.includes(data.departmentId)
				: false,
		[position, data.departmentId]
	);

	const {
		rows: certificatesRows,
		columns: certificatesColumns,
		handleSortColumnChange: handleCertificatesSortChange,
		sortColumns: certificatesSortColumns,
		isDataLoading: isLoadingCertificates,
		userCertificatesGridPagination,
		isFormVisible: isCertificateFormVisible,
		handlePopupToggle: handleCertificateFormToggle,
	} = useCertificatesGridState(isCertificatesGridVisible);

	const userValues = useMemo(
		() =>
			userId
				? getCertificateFormValuesFromPersonalData({
						personalData: data,
						userId,
						departmentTypes,
						roleTypes,
				  })
				: null,
		[data, userId, departmentTypes, roleTypes]
	);

	return (
		<Loader
			isLoading={
				isLoadingCertificates || isLoadingPersonalData || isLoadingEducations
			}
			size="small"
		>
			<EducationTabWrapper
				isGridRestricted={isCertificatesGridVisible}
				hasAddCertificateButton={isFullAccess(accessLevel)}
			>
				<EducationsGrid
					rows={educationRows}
					columns={educationColumns}
					pagination={educationsPagination}
					handleSortColumnChange={handleEducationsSortChange}
					sortColumns={educationSortColumns}
					handlePopupToggle={handleEducationFormToggle}
				/>
				{isCertificatesGridVisible && (
					<CertificatesGrid
						rows={certificatesRows}
						columns={certificatesColumns}
						handleSortColumnChange={handleCertificatesSortChange}
						sortColumns={certificatesSortColumns}
						pagination={userCertificatesGridPagination}
						handleCertificateFormToggle={handleCertificateFormToggle}
					/>
				)}
				{isCertificatesGridVisible &&
					isCertificateFormVisible &&
					userValues && (
						<CertificateForm
							initialValues={userValues}
							togglePopOver={handleCertificateFormToggle}
							isPopOverVisible={isCertificateFormVisible}
						/>
					)}

				{isEducationFormVisible && (
					<EducationForm
						closePopup={handleEducationFormToggle}
						education={chosenEducation}
					/>
				)}
			</EducationTabWrapper>
		</Loader>
	);
}
