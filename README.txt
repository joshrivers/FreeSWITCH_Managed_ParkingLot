Usage
-----

Console:
managed ParkingLotList

Dialplan:
	<extension name="parking_lot">
      <condition field="destination_number" expression="^105\d$">
	    <action application="set" data="parking_lot_entrance_number=1050" />
	    <action application="set" data="parking_lot_first_space=1051" />
	    <action application="set" data="parking_lot_spaces_count=9" />
		<action application="managed" data="ParkingLot"/>
      </condition>
    </extension>