import ba from '@/assets/departments/ba.png';
import devops from '@/assets/departments/devops.png';
import hr from '@/assets/departments/hr.png';
import java from '@/assets/departments/java.png';
import js from '@/assets/departments/js.png';
import marketing from '@/assets/departments/marketing.png';
import mobile from '@/assets/departments/mobile.png';
import net from '@/assets/departments/net.png';
import php from '@/assets/departments/php.png';
import pmo from '@/assets/departments/pmo.png';
import python from '@/assets/departments/python.png';
import qa from '@/assets/departments/qa.png';
import sales from '@/assets/departments/sales.png';
import { DEPARTMENT_IMAGES } from '@/helpers/getDepartmentsImage';

describe('Helpers object DEPARTMENT_IMAGES', () => {
	it('Should have the correct image for each department', () => {
		expect(DEPARTMENT_IMAGES.javascript).toBe(js);
		expect(DEPARTMENT_IMAGES.java).toBe(java);
		expect(DEPARTMENT_IMAGES['.net']).toBe(net);
		expect(DEPARTMENT_IMAGES.python).toBe(python);
		expect(DEPARTMENT_IMAGES.qa).toBe(qa);
		expect(DEPARTMENT_IMAGES.hr).toBe(hr);
		expect(DEPARTMENT_IMAGES.marketing).toBe(marketing);
		expect(DEPARTMENT_IMAGES.pmo).toBe(pmo);
		expect(DEPARTMENT_IMAGES.sales).toBe(sales);
		expect(DEPARTMENT_IMAGES.devops).toBe(devops);
		expect(DEPARTMENT_IMAGES.php).toBe(php);
		expect(DEPARTMENT_IMAGES.mobile).toBe(mobile);
		expect(DEPARTMENT_IMAGES.ba).toBe(ba);
	});
});
