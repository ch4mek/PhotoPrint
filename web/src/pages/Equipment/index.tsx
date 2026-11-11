import { useConfiguration } from '@/context/index';

import { EquipmentPageContent } from './components/EquipmentPageContent';

export function EquipmentPage() {
	const { isLoading: loading } = useConfiguration();

	if (loading) {
		return null;
	}

	return <EquipmentPageContent />;
}
