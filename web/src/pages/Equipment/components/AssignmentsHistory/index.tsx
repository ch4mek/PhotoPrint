import { EquipmentProvider } from '@/context/EquipmentContext/Equipment';
import { EquipmentForm } from '@/forms/EquipmentForm';
import { useShowModal } from '@/pages/hooks/useShowModal';

import { AssignmentsHistoryGrid } from './components/AssignmentsHistoryGrid';

export function AssignmentsHistory() {
	const { id, handleOpen, handleClose, isOpen } = useShowModal();

	return (
		<>
			<AssignmentsHistoryGrid handleOpen={handleOpen} />
			{isOpen && (
				<EquipmentProvider>
					<EquipmentForm id={id as string} onClose={handleClose} />
				</EquipmentProvider>
			)}
		</>
	);
}
