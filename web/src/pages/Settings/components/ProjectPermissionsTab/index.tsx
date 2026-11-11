import {
	LoaderProvider,
	useLoaderContext,
} from '@mms/mms-ui-library/components/ManagedLoader';

import { Loader } from '@/components/Loader';

import { AddBillableWorkingHoursPermissionsButton } from './components/AddBillableWorkingHoursPermissionsButton';
import { BillableWorkingHoursPermissionsGrid } from './components/BillableWorkingHoursPermissionsGrid';
import { BillableWorkingHoursPermissionsGridWrapper } from './styles';

function ProjectPermissionsTabInner() {
	const { isLoading } = useLoaderContext();

	return (
		<Loader isLoading={isLoading} loaderType="soft">
			<BillableWorkingHoursPermissionsGridWrapper>
				<AddBillableWorkingHoursPermissionsButton />
				<BillableWorkingHoursPermissionsGrid />
			</BillableWorkingHoursPermissionsGridWrapper>
		</Loader>
	);
}

export function ProjectPermissionsTab() {
	return (
		<LoaderProvider>
			<ProjectPermissionsTabInner />
		</LoaderProvider>
	);
}
